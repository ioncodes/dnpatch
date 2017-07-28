namespace dnpatch.Context
{
    /// <summary>
    /// Partial Assembly
    /// </summary>
    public partial class Assembly
	{
        /// <summary>
        /// Model context
        /// </summary>
        /// <seealso cref="dnpatch.Model.Model" />
        public class ModelContext : Model.Model
		{
            internal ModelContext(Types.Assembly assembly) : base(assembly)
			{
				Assembly = assembly;
			}
		}
	}
}
