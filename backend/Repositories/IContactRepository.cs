using PhoneBook.Api.Models;

namespace PhoneBook.Api.Repositories;

public interface IContactRepository
{
    Task<(List<Contact> Items, int TotalCount)> GetAll(int page, int pageSize);
    Task<Contact?> GetById(int id);
    Task<(List<Contact> Items, int TotalCount)> Search(string searchTerm, int page, int pageSize);
    Task<Contact> Create(Contact contact);
    Task<Contact?> Update(int id, Contact contact);
    Task<Contact?> Delete(int id);
}
