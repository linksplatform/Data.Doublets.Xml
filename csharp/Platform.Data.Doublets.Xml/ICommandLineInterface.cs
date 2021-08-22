#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

namespace Platform.Data.Doublets.Xml
{
    /// <summary>
    /// <para>
    /// Defines the command line interface.
    /// </para>
    /// <para></para>
    /// </summary>
    public interface ICommandLineInterface
    {
        /// <summary>
        /// <para>
        /// Runs the args.
        /// </para>
        /// <para></para>
        /// </summary>
        /// <param name="args">
        /// <para>The args.</para>
        /// <para></para>
        /// </param>
        void Run(params string[] args);
    }
}
