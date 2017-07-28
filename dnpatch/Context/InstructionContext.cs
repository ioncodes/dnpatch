namespace dnpatch.Context
{
	public partial class Assembly
	{
		public class InstructionContext : Model.Model
		{
			internal InstructionContext(Types.Assembly assembly) : base(assembly)
			{
				_assembly = assembly;
			}
		}
	}
}
