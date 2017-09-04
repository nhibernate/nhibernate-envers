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

		public IRevisionInfoGenerator RevisionInfoGenerator { get; }
		public XDocument RevisionInfoXmlMapping { get; }
		public RevisionInfoQueryCreator RevisionInfoQueryCreator { get; }
		public XElement RevisionInfoRelationMapping { get; }
		public RevisionInfoNumberReader RevisionInfoNumberReader { get; }
		public ModifiedEntityNamesReader ModifiedEntityNamesReader { get; }
		public string RevisionInfoEntityName { get; }
		public System.Type RevisionInfoClass { get; }
		public PropertyData RevisionInfoTimestampData { get; }
	}
}