// --------------------------------------------------------------------------------------------------------------------
// <copyright file="C8WindowsContainer.cs" company="AlFranco">
//   Albert Rodriguez Franco 2013
// </copyright>
// <summary>
//   Defines the C8WindowsContainer type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace C8POC.WinFormsUI
{
    using Autofac;

    using C8POC.Interfaces;
    using C8POC.WinFormsUI.Services;

    /// <summary>
    /// Container to resolve instances from the Windows Version of C8POC
    /// </summary>
    public class C8WindowsContainer : ContainerBuilder
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="C8WindowsContainer"/> class.
        /// </summary>
        public C8WindowsContainer()
        {
            this.RegisterType<C8Engine>().As<C8Engine>();
            this.RegisterType<C8MachineState>().As<IMachineState>();
            this.RegisterType<OpcodeProcessor>().As<IOpcodeProcessor>();
            this.RegisterType<WindowsConfigurationService>().As<IConfigurationService>();
            this.RegisterType<WindowsPluginService>().As<IPluginService>();
        }

        ///// <summary>
        ///// Initializes a new instance of the <see cref="C8WindowsContainer"/> class.
        ///// </summary>
        //public C8WindowsContainer()
        //{
        //    this.RegisterType<IMachineState, C8MachineState>();
        //    this.RegisterType<IOpcodeProcessor, OpcodeProcessor>();
        //    this.RegisterType<IConfigurationService, WindowsConfigurationService>();
        //    this.RegisterType<IPluginService, WindowsPluginService>();
        //}
    }
}
