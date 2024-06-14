namespace Fanex.Athena.Common.Model.DTO
{
    public class ProductOption
    {
        public string Code { get; set; }

        public string Name { get; set; }

        public bool Status { get; set; }

        public override int GetHashCode()
        {
            return this.Code.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            var item = obj as ProductOption;

            if (item == null)
            {
                return false;
            }

            return this.Code.Equals(item.Code, StringComparison.OrdinalIgnoreCase);
        }
    }
}
