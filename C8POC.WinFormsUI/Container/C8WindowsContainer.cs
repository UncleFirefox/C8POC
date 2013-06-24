// --------------------------------------------------------------------------------------------------------------------
// <copyright file="C8WindowsContainer.cs" company="AlFranco">
//   Albert Rodriguez Franco 2013
// </copyright>
// <summary>
//   Defines the C8WindowsContainer type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace C8POC.WinFormsUI.Container
{
    using Autofac;
    using Autofac.Extras.DynamicProxy2;

    using C8POC.Interfaces;
    using C8POC.WinFormsUI.Disassembly;
    using C8POC.WinFormsUI.Forms;
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
            this.RegisterType<WindowsConfigurationService>().As<IConfigurationService>();
            this.RegisterType<WindowsPluginService>().As<IPluginService>();
            this.RegisterType<OpcodeProcessor>().As<IOpcodeProcessor>();
        }

        /// <summary>
        /// Enables the disassembler with the given container
        /// </summary>
        /// <param name="disassemblerForm">
        /// The disassembler form.
        /// </param>
        public void EnableDisassembler(DisassemblerForm disassemblerForm)
        {
            this.Register(x => new MachineDisassemblerInterceptor(disassemblerForm));

            this.RegisterType<OpcodeProcessor>()
                .As<IOpcodeProcessor>()
                .EnableInterfaceInterceptors()
                .InterceptedBy(typeof(MachineDisassemblerInterceptor));
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
