namespace dnpatch.Context
{
	public partial class Assembly
	{
        public class ModelContext : Model.Model
		{
            internal ModelContext(Types.Assembly assembly) : base(assembly)
			{
				_assembly = assembly;
			}
		}
	}
}
