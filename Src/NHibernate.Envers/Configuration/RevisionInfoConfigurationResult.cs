using System.Xml.Linq;
using NHibernate.Envers.Entities;
using NHibernate.Envers.RevisionInfo;

namespace NHibernate.Envers.Configuration
{
	public class RevisionInfoConfigurationResult 
	{
		public RevisionInfoConfigurationResult(IRevisionInfoGenerator revisionInfoGenerator,
												XDocument revisionInfoXmlMapping, 
												RevisionInfoQueryCreator revisionInfoQueryCreator,
												XElement revisionInfoRelationMapping,
												RevisionInfoNumberReader revisionInfoNumberReader,
												ModifiedEntityNamesReader modifiedEntityNamesReader,
												string revisionInfoEntityName,
												System.Type revisionInfoClass,
												PropertyData revisionInfoTimestampData) 
		{
			RevisionInfoGenerator = revisionInfoGenerator;
			RevisionInfoXmlMapping = revisionInfoXmlMapping;
			RevisionInfoQueryCreator = revisionInfoQueryCreator;
			RevisionInfoRelationMapping = revisionInfoRelationMapping;
			RevisionInfoNumberReader = revisionInfoNumberReader;
			ModifiedEntityNamesReader = modifiedEntityNamesReader;
			RevisionInfoEntityName = revisionInfoEntityName;
			RevisionInfoClass = revisionInfoClass;
			RevisionInfoTimestampData = revisionInfoTimestampData;
		}

		public IRevisionInfoGenerator RevisionInfoGenerator { get; private set; }
		public XDocument RevisionInfoXmlMapping { get; private set; }
		public RevisionInfoQueryCreator RevisionInfoQueryCreator { get; private set; }
		public XElement RevisionInfoRelationMapping { get; private set; }
		public RevisionInfoNumberReader RevisionInfoNumberReader { get; private set; }
		public ModifiedEntityNamesReader ModifiedEntityNamesReader { get; private set; }
		public string RevisionInfoEntityName { get; private set; }
		public System.Type RevisionInfoClass { get; private set; }
		public PropertyData RevisionInfoTimestampData { get; private set; }
	}
}