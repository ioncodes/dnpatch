namespace dnpatch
{
	public partial class Assembly
	{
        public class ModelContext : Model
		{
            internal ModelContext(Assembly assembly) : base(assembly)
			{
				_assembly = assembly;
			}
		}
	}
}
