using System;
using System.Data;
using System.IO;
using System.Reflection;
using System.Text;
//using log4net;
using NHibernate;
using NHibernate.Mapping.Attributes;
using Spring.Data.NHibernate;
using NHibernate.Tool.hbm2ddl;
using System.Collections.ObjectModel;

namespace NHibernate.Envers.Utils
{
    /// <summary>
    /// An IFactoryObject that creates a local Hibernate SESSION_FACTORY instance.
    /// Behaves like a SessionFactory instance when used as bean reference,
    /// e.g. for HibernateTemplate's "SessionFactory" property.
    /// </summary>
    /// <author>Steinar Dragsnes (.NET)</author>
    public class AttributesSessionFactoryObject : LocalSessionFactoryObject
    {
        #region Fields
        private string hibernateConfigFile;
        private string appPath = AppDomain.CurrentDomain.SetupInformation.ApplicationBase;
        private string hbmXmlDumpFile = "dump.hbm.xml";
        private string sqlExportDumpFile = "export.sql";
        private string insertsFile = "mssql_inserts.sql";
        private bool doInserts = false;
        private bool doHbmXmlDump = true;
        private bool rebuildDB = false;
        private Assembly assembly;
        #endregion

        #region Constructor(s)
        /// <summary>
        /// Default empty constructor useful for DI.
        /// </summary>
        public AttributesSessionFactoryObject() : base() { }
        #endregion

        #region Properties
        /// <summary>
        /// The path to a hibernate configuration file.
        /// </summary>
        public string HibernateConfigFile
        {
            get { return hibernateConfigFile; }
            set { hibernateConfigFile = value; }
        }

        /// <summary>
        /// If the generated mappings should be dumped to a file.
        /// </summary>
        public bool DoHbmXmlDump
        {
            get { return doHbmXmlDump; }
            set { doHbmXmlDump = value; }
        }

        /// <summary>
        /// Output file storing all mappings extracted from provided assembly.
        /// </summary>
        public string HbmXmlDumpFile
        {
            get { return hbmXmlDumpFile; }
            set { hbmXmlDumpFile = value; }
        }

        /// <summary>
        /// A file containing insert statements to populate the DB with default data whenever the database is rebuilt (dropped and re-created).
        /// </summary>
        public string InsertsFile
        {
            get { return insertsFile; }
            set { insertsFile = value; }
        }

        /// <summary>
        /// Flag indicating whether a sql-file containing insert statements should be executed.
        /// </summary>
        public bool DoInserts
        {
            get { return doInserts; }
            set { doInserts = value; }
        }


        public string AppPath
        {
            get { return appPath; }
            set { appPath = value; }
        }

        public string SqlExportDumpFile
        {
            get { return sqlExportDumpFile; }
            set { sqlExportDumpFile = value; }
        }

        /// <summary>
        /// Flag indicating that the database should be rebuilt (dropped and re-created
        /// </summary>
        public bool RebuildDB
        {
            get { return rebuildDB; }
            set { rebuildDB = value; }
        }

        /// <summary>
        /// The assembly containing the mappings
        /// </summary>
        public Assembly Assembly
        {
            set { assembly = value; }
        }
        #endregion

        #region Overloaded-Methods
        /// <summary>
        /// Overridden and implemented to perform custom
        /// post-processing of the Configuration object after this FactoryObject
        /// performed its default initialization.
        /// </summary>
        /// <param name="config">The current configuration object.</param>
        protected override void PostProcessConfiguration(NHibernate.Cfg.Configuration config)
        {
            Initialize(config);
        }

        /// <summary>
        /// Overridden and implemented to perform custom initialization
        /// of the SessionFactory instance, creating it via the given Configuration
        /// object that got prepared by this LocalSessionFactoryObject.
        /// </summary>
        protected override ISessionFactory NewSessionFactory(NHibernate.Cfg.Configuration config)
        {
            ISessionFactory sessionFactory = base.NewSessionFactory(config);
            SchemaExport(config, sessionFactory);
            return sessionFactory;
        }
        #endregion

        #region Methods
        /// <summary>
        /// Initialize the configuration based on the provided assembly.
        /// </summary>
        /// <param name="config">The NH-configuration instance.</param>
        private NHibernate.Cfg.Configuration Initialize(NHibernate.Cfg.Configuration config)
        {
            // Gather information from this assembly 
            //MemoryStream stream = HbmSerializer.Default.Serialize(assembly);
            //stream.Position = 0;
            //config.AddInputStream(stream); // Send the Mapping information to NHibernate Configuration
            AddInputStreamsForAssembly(config, assembly);

            //stream.Close();

            return config;
        }

        private static void AddInputStreamsForAssembly(NHibernate.Cfg.Configuration _configuration, Assembly asm)
        {
            Type[] types = GetTypesWithClassAttribute(asm);
            foreach (Type t in types)
            {
                HbmSerializer.Default.HbmAssembly = asm.GetName().Name;
                HbmSerializer.Default.HbmNamespace = t.Namespace;
                // Use NHibernate.Mapping.Attributes to create information about our entities
                HbmSerializer.Default.Validate = true; // Enable validation (optional)

                System.IO.MemoryStream stream = new System.IO.MemoryStream();
                Console.WriteLine(t.Name);
                HbmSerializer.Default.Serialize(stream, t);
                stream.Position = 0;
                Console.WriteLine((new StreamReader(stream)).ReadToEnd());
                Console.WriteLine();
                stream.Position = 0;
                //DumpHbmXmlStreamToFile(stream, Path.Combine(appPath, hbmXmlDumpFile));
                //stream.Position = 0;
                _configuration.AddInputStream(stream);
                stream.Close();
            }
        }

        private static Type[] GetTypesWithClassAttribute(Assembly asm)
        {
            Type[] types = asm.GetTypes();
            Collection<Type> classTypes = new Collection<Type>();

            foreach (Type t in types)
            {
                //Console.WriteLine("type:" + t.Name);
                Type tAtt = typeof(ClassAttribute);
                ClassAttribute a = (ClassAttribute)Attribute.GetCustomAttribute(t, tAtt);
                if (a != null)
                {
                    //Console.WriteLine("Are clasa: " + a.Name);
                    classTypes.Add(t);
                }
            }

            Type[] classTypesArray = new Type[classTypes.Count];
            classTypes.CopyTo(classTypesArray, 0);
            return classTypesArray;
        }


        /// <summary>
        /// Export generated database schema to DB.
        /// </summary>
        protected void SchemaExport(NHibernate.Cfg.Configuration config, ISessionFactory sessionFactory)
        {
            if (rebuildDB)
            {
                SchemaExport se = new SchemaExport(config);
                se.SetOutputFile(Path.Combine(appPath, sqlExportDumpFile));
                se.Create(true, true);
            }

            if (doInserts)
            {
                // Use sql insertstatements to populate your DB!
                ISession sess = null;
                try
                {
                    // open a session just for populating the db, it will be closed when done....
                    sess = sessionFactory.OpenSession();
                    IDbCommand com = sess.Connection.CreateCommand();
                    com.CommandText = ReadInsertStatementsToStream(insertsFile).ToString();
                    com.ExecuteNonQuery();
                }
                catch (ReadInsertStatementsFromFileToStreamException e) { throw e; }
                catch (Exception e) { throw e; }
                finally { if (sess != null) sess.Close(); }
            }
        }

        /// <summary>
        /// Dump hbm.xml generated stream to file.
        /// </summary>
        /// <param name="fromStream"></param>
        /// <param name="file"></param>
        private void DumpHbmXmlStreamToFile(Stream fromStream, String file)
        {
            BinaryWriter bw = null;

            try
            {
                Stream toStream = File.Create(file);
                BinaryReader br = new BinaryReader(fromStream);
                bw = new BinaryWriter(toStream);
                bw.Write(br.ReadBytes((int)fromStream.Length));
                bw.Flush();
            }
            // Simply trying to dump data to a file. If this operation fails, then continue, nothing crucial...
            catch (Exception) { }
            finally { if (bw != null) bw.Close(); }
        }

        /// <summary>
        /// Populate database based on a file holding the insert statements.
        /// </summary>
        /// <param name="insertsFile"></param>
        /// <returns></returns>
        private static StringBuilder ReadInsertStatementsToStream(String insertsFile)
        {
            StringBuilder sb;
            BinaryReader br = null;
            try
            {
                //default to the app directory if no path was specified in the inserts file name
                //apparently the pure filename can not be resolved when running as a service, so 
                //we have to build an absolute path here
                if (String.IsNullOrEmpty(Path.GetDirectoryName(insertsFile)))
                {
                    insertsFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, insertsFile);
                }
                Stream fileStream = File.OpenRead(insertsFile);
                br = new BinaryReader(fileStream);
                int length = (int)fileStream.Length;
                sb = new StringBuilder(length);
                sb.Insert(0, br.ReadChars(length));
            }
            //Rethrow any of the exceptions that may occurr
            catch (Exception ex)
            {
                throw new ReadInsertStatementsFromFileToStreamException("Couldn't read or file doesn't exist: [" + insertsFile + "]", ex.GetBaseException());
            }
            finally { if (br != null) br.Close(); }

            return sb;
        }
        #endregion
    }

    #region Exceptions
    /// <summary>
    /// Read insert statements from file exception.
    /// </summary>
    public class ReadInsertStatementsFromFileToStreamException : ApplicationException
    {

        /// <summary>
        /// Constructor for constructing the exception.
        /// </summary>
        /// <param name="message">The error message.</param>
        /// <param name="innerException">The wrappedinner exception.</param>
        public ReadInsertStatementsFromFileToStreamException(string message, Exception innerException) : base(message, innerException) { }
    }
    #endregion
}