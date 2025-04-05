using System.Reflection;

namespace Readify.UnitTests.Architectures
{
    public class ArchitectureTests
    {
        private const string APIProjectName = "Readify.API";
        private const string ApplicationProjectName = "Readify.Application";
        private const string InfrastructureProjectName = "Readify.Infrastructure";
        private const string CrossCuttingProjectName = "Readify.CrossCutting";

        // Obtém os assemblies dos projetos
        private static readonly Assembly ApiAssembly = typeof(Readify.API.Features.Books.V1.Controllers.BooksController).Assembly; // Substitua "Startup" pelo namespace principal do seu projeto API
        private static readonly Assembly ApplicationAssembly = typeof(Readify.Application.Features.Books.V1.IBooksAppServices).Assembly; // Substitua "SomeClass" por uma classe no projeto Application
        private static readonly Assembly InfrastructureAssembly = typeof(Readify.Infrastructure.Commons.DatabaseContexts.V1.ReadifyDatabaseContext).Assembly; // Substitua "SomeClass" por uma classe no projeto Infrastructure
        private static readonly Assembly CrossCuttingAssembly = typeof(Readify.CrossCutting.DependencyInjection.IoCExtension).Assembly; // Substitua "SomeClass" por uma classe no projeto CrossCutting

        [Fact]
        public void ApiProject_Should_Not_References_InfrastructureProject()
        {
            // Define a regra: O projeto API não deve referenciar o projeto Infrastructure
            var references = ApiAssembly.GetReferencedAssemblies();
            bool hasDependency = references.Any(a => a.Name == InfrastructureProjectName);

            Assert.True(!hasDependency, $"The {APIProjectName} project shouldn't references {InfrastructureProjectName} project.");
        }

        [Fact]
        public void ApiProject_Should_References_ApplicationProject()
        {
            // Define a regra: O projeto API deve referenciar o projeto Application
            var references = ApiAssembly.GetReferencedAssemblies();
            bool hasDependency = references.Any(a => a.Name == ApplicationProjectName);

            Assert.True(hasDependency, $"The {APIProjectName} project should references {ApplicationProjectName} project.");
        }

        [Fact]
        public void ApiProject_Should_References_CrossCuttingProject()
        {
            // Define a regra: O projeto API deve referenciar o projeto CrossCutting
            var references = ApiAssembly.GetReferencedAssemblies();
            bool hasDependency = references.Any(a => a.Name == CrossCuttingProjectName);

            Assert.True(hasDependency, $"The {APIProjectName} project should references {CrossCuttingProjectName} project.");
        }

        [Fact]
        public void ApplicationProject_Should_Not_References_AnyProject()
        {
            // Define a regra: O projeto API deve referenciar o projeto Application
            var references = ApplicationAssembly.GetReferencedAssemblies();
            bool hasDependency = references.Any(a => a.Name == APIProjectName || a.Name == InfrastructureProjectName || a.Name == CrossCuttingProjectName);

            Assert.True(!hasDependency, $"The {ApplicationProjectName} project shouldn't references any project.");
        }

        [Fact]
        public void InfrastructureProject_Should_Not_References_APIProject()
        {
            // Define a regra: O projeto API deve referenciar o projeto Application
            var references = InfrastructureAssembly.GetReferencedAssemblies();
            bool hasDependency = references.Any(a => a.Name == APIProjectName);

            Assert.True(!hasDependency, $"The {InfrastructureProjectName} project shouldn't references {APIProjectName} project.");
        }

        [Fact]
        public void InfrastructureProject_Should_Not_References_CrossCuttingProject()
        {
            // Define a regra: O projeto API deve referenciar o projeto Application
            var references = InfrastructureAssembly.GetReferencedAssemblies();
            bool hasDependency = references.Any(a => a.Name == CrossCuttingProjectName);

            Assert.True(!hasDependency, $"The {InfrastructureProjectName} project shouldn't references {CrossCuttingProjectName} project.");
        }

        [Fact]
        public void InfrastructureProject_Should_References_ApplicationProject()
        {
            // Define a regra: O projeto API deve referenciar o projeto Application
            var references = InfrastructureAssembly.GetReferencedAssemblies();
            bool hasDependency = references.Any(a => a.Name == ApplicationProjectName);

            Assert.True(hasDependency, $"The {InfrastructureProjectName} project should references {ApplicationProjectName} project.");
        }

        [Fact]
        public void CrossCuttingProject_Should_References_ApplicationProject()
        {
            // Define a regra: O projeto API deve referenciar o projeto Application
            var references = CrossCuttingAssembly.GetReferencedAssemblies();
            bool hasDependency = references.Any(a => a.Name == ApplicationProjectName);

            Assert.True(hasDependency, $"The {CrossCuttingProjectName} project should references {ApplicationProjectName} project.");
        }

        [Fact]
        public void CrossCuttingProject_Should_References_InfrastructureProject()
        {
            // Define a regra: O projeto API deve referenciar o projeto Application
            var references = CrossCuttingAssembly.GetReferencedAssemblies();
            bool hasDependency = references.Any(a => a.Name == InfrastructureProjectName);

            Assert.True(hasDependency, $"The {CrossCuttingProjectName} project should references {InfrastructureProjectName} project.");
        }

        [Fact]
        public void CrossCuttingProject_Should_Not_References_APIProject()
        {
            // Define a regra: O projeto API deve referenciar o projeto Application
            var references = CrossCuttingAssembly.GetReferencedAssemblies();
            bool hasDependency = references.Any(a => a.Name == APIProjectName);

            Assert.True(!hasDependency, $"The {CrossCuttingProjectName} project shouldn't references {APIProjectName} project.");
        }
    }
}
