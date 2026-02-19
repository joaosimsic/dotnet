using FluentAssertions;
using Moq;
using PhoneBook.Api.DTOs;
using PhoneBook.Api.Models;
using PhoneBook.Api.Repositories;
using PhoneBook.Api.Services;

namespace PhoneBook.Api.Tests.Services;

public class ContactServiceTests
{
    private readonly Mock<IContactRepository> _repositoryMock;
    private readonly Mock<IDeletionLogService> _deletionLogServiceMock;
    private readonly ContactService _service;

    public ContactServiceTests()
    {
        _repositoryMock = new Mock<IContactRepository>();
        _deletionLogServiceMock = new Mock<IDeletionLogService>();
        _service = new ContactService(_repositoryMock.Object, _deletionLogServiceMock.Object);
    }

    [Fact]
    public async Task GetAllAsync_ReturnsPagedResult()
    {
        var contacts = new List<Contact>
        {
            CreateContact(1, "Alice", 25),
            CreateContact(2, "Bob", 30)
        };

        _repositoryMock.Setup(r => r.GetAll(1, 10))
            .ReturnsAsync((contacts, 2));

        var result = await _service.GetAllAsync(1, 10);

        result.Items.Should().HaveCount(2);
        result.TotalCount.Should().Be(2);
        result.Page.Should().Be(1);
        result.PageSize.Should().Be(10);
        result.TotalPages.Should().Be(1);
    }

    [Fact]
    public async Task GetAllAsync_CalculatesTotalPagesCorrectly()
    {
        var contacts = new List<Contact> { CreateContact(1, "Alice", 25) };

        _repositoryMock.Setup(r => r.GetAll(1, 10))
            .ReturnsAsync((contacts, 25));

        var result = await _service.GetAllAsync(1, 10);

        result.TotalPages.Should().Be(3);
    }

    [Fact]
    public async Task GetByIdAsync_ReturnsContact_WhenFound()
    {
        var contact = CreateContact(1, "Alice", 25);
        _repositoryMock.Setup(r => r.GetById(1)).ReturnsAsync(contact);

        var result = await _service.GetByIdAsync(1);

        result.Should().NotBeNull();
        result!.Id.Should().Be(1);
        result.Name.Should().Be("Alice");
        result.Age.Should().Be(25);
    }

    [Fact]
    public async Task GetByIdAsync_ReturnsNull_WhenNotFound()
    {
        _repositoryMock.Setup(r => r.GetById(999)).ReturnsAsync((Contact?)null);

        var result = await _service.GetByIdAsync(999);

        result.Should().BeNull();
    }

    [Fact]
    public async Task SearchAsync_ReturnsMatchingContacts()
    {
        var contacts = new List<Contact> { CreateContact(1, "Alice", 25) };
        _repositoryMock.Setup(r => r.Search("Alice", 1, 10))
            .ReturnsAsync((contacts, 1));

        var result = await _service.SearchAsync("Alice", 1, 10);

        result.Items.Should().HaveCount(1);
        result.Items[0].Name.Should().Be("Alice");
    }

    [Fact]
    public async Task CreateAsync_CreatesAndReturnsContact()
    {
        var dto = new CreateContactDto("Alice", 25, new List<string> { "123-456-7890" });
        var createdContact = CreateContact(1, "Alice", 25, "123-456-7890");

        _repositoryMock.Setup(r => r.Create(It.IsAny<Contact>()))
            .ReturnsAsync(createdContact);

        var result = await _service.CreateAsync(dto);

        result.Id.Should().Be(1);
        result.Name.Should().Be("Alice");
        result.Age.Should().Be(25);
        result.Phones.Should().HaveCount(1);
        result.Phones[0].PhoneNumber.Should().Be("123-456-7890");
    }

    [Fact]
    public async Task UpdateAsync_ReturnsUpdatedContact_WhenFound()
    {
        var dto = new UpdateContactDto("Bob", 35, new List<string> { "999-888-7777" });
        var updatedContact = CreateContact(1, "Bob", 35, "999-888-7777");

        _repositoryMock.Setup(r => r.Update(1, It.IsAny<Contact>()))
            .ReturnsAsync(updatedContact);

        var result = await _service.UpdateAsync(1, dto);

        result.Should().NotBeNull();
        result!.Name.Should().Be("Bob");
        result.Age.Should().Be(35);
    }

    [Fact]
    public async Task UpdateAsync_ReturnsNull_WhenNotFound()
    {
        var dto = new UpdateContactDto("Bob", 35, new List<string> { "999-888-7777" });
        _repositoryMock.Setup(r => r.Update(999, It.IsAny<Contact>()))
            .ReturnsAsync((Contact?)null);

        var result = await _service.UpdateAsync(999, dto);

        result.Should().BeNull();
    }

    [Fact]
    public async Task DeleteAsync_ReturnsDeletedContact_WhenFound()
    {
        var contact = CreateContact(1, "Alice", 25);
        _repositoryMock.Setup(r => r.Delete(1)).ReturnsAsync(contact);
        _deletionLogServiceMock.Setup(s => s.LogDeletionAsync(It.IsAny<ContactDto>()))
            .Returns(Task.CompletedTask);

        var result = await _service.DeleteAsync(1);

        result.Should().NotBeNull();
        result!.Name.Should().Be("Alice");
        _deletionLogServiceMock.Verify(s => s.LogDeletionAsync(It.IsAny<ContactDto>()), Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_ReturnsNull_WhenNotFound()
    {
        _repositoryMock.Setup(r => r.Delete(999)).ReturnsAsync((Contact?)null);

        var result = await _service.DeleteAsync(999);

        result.Should().BeNull();
        _deletionLogServiceMock.Verify(s => s.LogDeletionAsync(It.IsAny<ContactDto>()), Times.Never);
    }

    private static Contact CreateContact(int id, string name, int age, params string[] phones)
    {
        return new Contact
        {
            Id = id,
            Name = name,
            Age = age,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            Phones = phones.Select((p, i) => new Phone
            {
                Id = i + 1,
                ContactId = id,
                PhoneNumber = p,
                CreatedAt = DateTime.UtcNow
            }).ToList()
        };
    }
}
