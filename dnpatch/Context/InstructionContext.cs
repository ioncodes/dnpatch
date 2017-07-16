namespace dnpatch
{
	public partial class Assembly
	{
		public class InstructionContext : Model
		{
			internal InstructionContext(Assembly assembly) : base(assembly)
			{
				_assembly = assembly;
			}
		}
	}
}
