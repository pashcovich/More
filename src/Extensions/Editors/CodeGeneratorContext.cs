﻿namespace More.VisualStudio.Editors
{
    using EnvDTE;
    using System;
    using System.Diagnostics.Contracts;

    /// <summary>
    /// Represents a code generation context.
    /// </summary>
    public class CodeGeneratorContext : IServiceProvider
    {
        private readonly string filePath;
        private readonly string fileContents;
        private readonly string defaultNamespace;
        private readonly IProgress<GeneratorProgress> progress;
        private readonly IServiceProvider serviceProvider;
        private readonly Lazy<DTE> dte;
        private readonly Lazy<ProjectItem> projectItem;

        /// <summary>
        /// Initializes a new instance of the <see cref="CodeGeneratorContext"/> class.
        /// </summary>
        /// <param name="filePath">The file path of the input file.</param>
        /// <param name="fileContents">The contents for the input file.</param>
        /// <param name="defaultNamespace">The default namespace for the input file.</param>
        /// <param name="progress">An object that can be used to report the <see cref="IProgress{T}">progress</see> of code generation.</param>
        /// <param name="serviceProvider">The underlying <see cref="IServiceProvider">service provider</see> for the context.</param>
        public CodeGeneratorContext( string filePath, string fileContents, string defaultNamespace, IProgress<GeneratorProgress> progress, IServiceProvider serviceProvider )
        {
            Arg.NotNullOrEmpty( filePath, nameof( filePath ) );
            Arg.NotNull( fileContents, nameof( fileContents ) );
            Arg.NotNullOrEmpty( defaultNamespace, nameof( defaultNamespace ) );
            Arg.NotNull( progress, nameof( progress ) );
            Arg.NotNull( serviceProvider, nameof( serviceProvider ) );

            this.filePath = filePath;
            this.fileContents = fileContents;
            this.defaultNamespace = defaultNamespace;
            this.progress = progress;
            this.serviceProvider = serviceProvider;
            dte = new Lazy<DTE>( this.serviceProvider.GetRequiredService<DTE> );
            projectItem = new Lazy<ProjectItem>( () => DesignTimeEnvironment.Solution.FindProjectItem( FilePath ) );
        }

        /// <summary>
        /// Gets the file path of the input file.
        /// </summary>
        /// <value>The input file path.</value>
        public string FilePath
        {
            get
            {
                Contract.Ensures( !string.IsNullOrEmpty( filePath ) );
                return filePath;
            }
        }

        /// <summary>
        /// Gets the input file contents.
        /// </summary>
        /// <value>The current input file contents.</value>
        public string FileContents
        {

            get
            {
                Contract.Ensures( fileContents != null );
                return fileContents;
            }
        }

        /// <summary>
        /// Gets the default namespace for the input file.
        /// </summary>
        /// <value>The default namespace associated with the input file.</value>
        public string DefaultNamespace
        {
            get
            {
                Contract.Ensures( !string.IsNullOrEmpty( defaultNamespace ) );
                return defaultNamespace;
            }
        }

        /// <summary>
        /// Gets an object that can be used to report code generation progress.
        /// </summary>
        /// <value>An object used to report <see cref="IProgress{T}">progress</see>.</value>
        public IProgress<GeneratorProgress> Progress
        {
            get
            {
                Contract.Ensures( progress != null );
                return progress;
            }
        }

        /// <summary>
        /// Gets the design-time environment (DTE) associated with input file.
        /// </summary>
        /// <value>The associated <see cref="DTE">design-time environment</see>.</value>
        public DTE DesignTimeEnvironment
        {
            get
            {
                Contract.Ensures( Contract.Result<DTE>() != null );
                return dte.Value;
            }
        }

        /// <summary>
        /// Gets the project associated with input file.
        /// </summary>
        /// <value>The associated <see cref="Project">project</see>.</value>
        public Project Project
        {
            get
            {
                Contract.Ensures( Contract.Result<Project>() != null );
                return ProjectItem.ContainingProject;
            }
        }

        /// <summary>
        /// Gets the project item for input file.
        /// </summary>
        /// <value>The <see cref="ProjectItem">project item</see> for the input file.</value>
        public ProjectItem ProjectItem
        {
            get
            {
                Contract.Ensures( Contract.Result<ProjectItem>() != null );
                return projectItem.Value;
            }
        }

        /// <summary>
        /// Returns the service returned with the specified service type.
        /// </summary>
        /// <param name="serviceType">The <see cref="Type">type</see> of service requested.</param>
        /// <returns>An instance of the requested <paramref name="serviceType">service type</paramref> or <c>null</c> if no match is found.</returns>
        public object GetService( Type serviceType )
        {
            Arg.NotNull( serviceType, nameof( serviceType ) );
            return serviceProvider.GetService( serviceType );
        }
    }
}
