using System.Xml;
using NHibernate.Envers.RevisionInfo;

namespace NHibernate.Envers.Configuration
{
	public class RevisionInfoConfigurationResult 
	{
		public RevisionInfoConfigurationResult(IRevisionInfoGenerator revisionInfoGenerator,
		                                       XmlDocument revisionInfoXmlMapping, 
		                                       RevisionInfoQueryCreator revisionInfoQueryCreator,
		                                       XmlElement revisionInfoRelationMapping,
		                                       RevisionInfoNumberReader revisionInfoNumberReader, 
		                                       string revisionInfoEntityName) 
		{
			RevisionInfoGenerator = revisionInfoGenerator;
			RevisionInfoXmlMapping = revisionInfoXmlMapping;
			RevisionInfoQueryCreator = revisionInfoQueryCreator;
			RevisionInfoRelationMapping = revisionInfoRelationMapping;
			RevisionInfoNumberReader = revisionInfoNumberReader;
			RevisionInfoEntityName = revisionInfoEntityName;
		}

		public IRevisionInfoGenerator RevisionInfoGenerator { get; private set; }
		public XmlDocument RevisionInfoXmlMapping { get; private set; }
		public RevisionInfoQueryCreator RevisionInfoQueryCreator { get; private set; }
		public XmlElement RevisionInfoRelationMapping { get; private set; }
		public RevisionInfoNumberReader RevisionInfoNumberReader { get; private set; }
		public string RevisionInfoEntityName { get; private set; }
	}
}