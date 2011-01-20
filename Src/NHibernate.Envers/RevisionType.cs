using System;

namespace NHibernate.Envers
{
	//rk - make enum instead
	public enum RevisionType
	{
		ADD,
		MOD,
		DEL
	}

	//[Serializable]
	//public class RevisionType 
	//{
	//    public static readonly RevisionType ADD = new RevisionType(0);
	//    public static readonly RevisionType MOD = new RevisionType(1);
	//    public static readonly RevisionType DEL = new RevisionType(2);
	//    public byte Representation {get; private set;}

	//    private RevisionType(byte representation)
	//    {
	//        Representation = representation;
	//    }

	//    public static RevisionType FromRepresentation(byte representation) 
	//    {
	//        switch (representation) 
	//        {
	//            case 0: return ADD;
	//            case 1: return MOD;
	//            case 2: return DEL;
	//        }

	//        throw new ArgumentOutOfRangeException("Unknown representation: " + representation);
	//    }
	//}
}
