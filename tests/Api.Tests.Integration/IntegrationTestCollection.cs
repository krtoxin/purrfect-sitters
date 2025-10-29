

using Xunit;

namespace Api.Tests.Integration;

[CollectionDefinition("Integration")]
public class IntegrationTestCollection : Xunit.ICollectionFixture<global::Tests.Common.IntegrationTestWebFactory> { }
