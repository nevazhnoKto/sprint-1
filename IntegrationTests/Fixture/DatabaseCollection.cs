

namespace IntegrationTests.Fixture
{
	/// <summary>
	/// Определяем коллекцию для общей фикстуры
	/// Все тестовые классы, использующие эту коллекцию, будут разделять один экземпляр фикстуры
	/// </summary>
	[CollectionDefinition("Database")]
	public class DatabaseCollection : ICollectionFixture<DatabaseFixture>
	{
		// Этот класс не требует реализации
		// Он нужен только для привязки [Collection] атрибута к фикстуре
	}
}
