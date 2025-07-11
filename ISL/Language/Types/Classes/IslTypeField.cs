namespace ISL.Language.Types.Classes
{
    internal class IslTypeField : IslTypeMember
    {
        public IslType FieldType = IslType.Null;
        public IslValue Value = IslValue.Null;

        public bool readOnly = false;
        public bool isUninitialised = false;

        public override string ToString()
        {
            return $"{(readOnly?"readonly ":"")}{FieldType} field (= {Value})";
        }
    }
}
