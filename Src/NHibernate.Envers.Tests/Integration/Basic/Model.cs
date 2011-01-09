namespace NHibernate.Envers.Tests.Integration.Basic
{
    public class BasicTestEntity1
    {
        public virtual int Id { get; set; }
        [Audited]
        public virtual string Str1 { get; set; }
        [Audited]
        public virtual long Long1 { get; set; }

        public override bool Equals(object obj)
        {
            var bte = obj as BasicTestEntity1;
            if (bte == null)
                return false;
            return (bte.Id == Id && string.Equals(bte.Str1, Str1) && bte.Long1 == Long1);
        }

        public override int GetHashCode()
        {
            return Id ^ Str1.GetHashCode() ^ Long1.GetHashCode();
        }
    }

    public class BasicTestEntity2
    {
        public virtual int Id { get; set; }
        [Audited]
        public virtual string Str1 { get; set; }
        public virtual string Str2 { get; set; }

        public override bool Equals(object obj)
        {
            var bte = obj as BasicTestEntity2;
            if (bte == null)
                return false;
            return (bte.Id == Id && string.Equals(bte.Str1, Str1) && string.Equals(bte.Str2, Str2));
        }

        public override int GetHashCode()
        {
            return Id ^ Str1.GetHashCode() ^ Str2.GetHashCode();
        }
    }

    public class BasicTestEntity3
    {
        public virtual int Id { get; set; }
        public virtual string Str1 { get; set; }
        public virtual string Str2 { get; set; }

        public override bool Equals(object obj)
        {
            var bte = obj as BasicTestEntity3;
            if (bte == null)
                return false;
            return (bte.Id == Id && string.Equals(bte.Str1, Str1) && string.Equals(bte.Str2, Str2));
        }

        public override int GetHashCode()
        {
            return Id ^ Str1.GetHashCode() ^ Str2.GetHashCode();
        }
    }

    [Audited]
    public class BasicTestEntity4
    {
        public virtual int Id { get; set; }
        public virtual string Str1 { get; set; }
        public virtual string Str2 { get; set; }

        public override bool Equals(object obj)
        {
            var bte = obj as BasicTestEntity4;
            if (bte == null)
                return false;
            return (bte.Id == Id && string.Equals(bte.Str1, Str1) && string.Equals(bte.Str2, Str2));
        }

        public override int GetHashCode()
        {
            return Id ^ Str1.GetHashCode() ^ Str2.GetHashCode();
        }
    }
}