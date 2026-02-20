import { useState, useEffect } from "react";
import { Plus, Search, X } from "lucide-react";
import { Button } from "./components/ui/button";
import { SearchBar } from "./components/SearchBar";
import { ContactList } from "./components/ContactList";
import { ContactForm } from "./components/ContactForm";
import { Pagination } from "./components/Pagination";
import { ConfirmDialog } from "./components/ConfirmDialog";
import { useContacts } from "./hooks/useContacts";
import { useConfirmDialog } from "./hooks/useConfirmDialog";
import type { Contact, CreateContactDto, UpdateContactDto } from "./types";

type ViewMode = "list" | "add" | "edit";

export default function App() {
  const {
    contacts,
    pagination,
    isLoading,
    error,
    loadContacts,
    createContact,
    updateContact,
    deleteContact,
    setPage,
    clearError,
  } = useContacts();

  const { dialogState, confirm, closeDialog } = useConfirmDialog();

  const [selectedContact, setSelectedContact] = useState<Contact | null>(null);
  const [viewMode, setViewMode] = useState<ViewMode>("list");
  const [searchQuery, setSearchQuery] = useState("");

  useEffect(() => {
    loadContacts();
  }, [loadContacts]);

  const handleSearch = () => {
    loadContacts(searchQuery);
  };

  const handleClearSearch = () => {
    setSearchQuery("");
    loadContacts("");
  };

  const handleAdd = () => {
    setSelectedContact(null);
    setViewMode("add");
  };

  const handleEdit = (contact: Contact) => {
    setSelectedContact(contact);
    setViewMode("edit");
  };

  const handleDelete = async (contact: Contact) => {
    const confirmed = await confirm(
      "Delete Contact",
      `Are you sure you want to delete "${contact.name}"? This action cannot be undone.`,
    );

    if (confirmed) {
      const success = await deleteContact(contact.id);
      if (success && selectedContact?.id === contact.id) {
        setSelectedContact(null);
      }
    }
  };

  const handleSubmit = async (data: CreateContactDto | UpdateContactDto) => {
    let success: boolean;

    if (viewMode === "edit" && selectedContact) {
      success = await updateContact(
        selectedContact.id,
        data as UpdateContactDto,
      );
    } else {
      success = await createContact(data as CreateContactDto);
    }

    if (success) {
      setViewMode("list");
      setSelectedContact(null);
    }
  };

  const handleCancel = () => {
    setViewMode("list");
    setSelectedContact(null);
  };

  return (
    <div className="min-h-screen bg-background">
      <div className="container mx-auto py-8 px-4 max-w-3xl">
        <header className="mb-6">
          <h1 className="text-3xl font-bold mb-2">Phone Book</h1>
          <p className="text-muted-foreground">Manage your contacts</p>
        </header>

        {error && (
          <div className="mb-4 p-4 bg-destructive/10 text-destructive rounded-lg flex items-center justify-between">
            <span>{error}</span>
            <Button
              variant="ghost"
              size="icon"
              onClick={clearError}
              aria-label="Dismiss error"
            >
              <X className="h-4 w-4" />
            </Button>
          </div>
        )}

        {viewMode === "list" ? (
          <>
            <div className="flex gap-2 mb-6">
              <div className="flex-1">
                <SearchBar
                  value={searchQuery}
                  onChange={setSearchQuery}
                  onSearch={handleSearch}
                />
              </div>
              <Button
                onClick={handleSearch}
                variant="secondary"
                aria-label="Search contacts"
              >
                <Search className="h-4 w-4 mr-2" />
                Search
              </Button>
              {searchQuery && (
                <Button
                  onClick={handleClearSearch}
                  variant="outline"
                  aria-label="Clear search"
                >
                  <X className="h-4 w-4" />
                </Button>
              )}
              <Button onClick={handleAdd} aria-label="Add new contact">
                <Plus className="h-4 w-4 mr-2" />
                Add
              </Button>
            </div>

            <ContactList
              contacts={contacts}
              selectedId={selectedContact?.id ?? null}
              onSelect={setSelectedContact}
              onEdit={handleEdit}
              onDelete={handleDelete}
              isLoading={isLoading}
            />

            <Pagination
              page={pagination.page}
              totalPages={pagination.totalPages}
              totalCount={pagination.totalCount}
              onPageChange={setPage}
              isLoading={isLoading}
            />
          </>
        ) : (
          <ContactForm
            contact={viewMode === "edit" ? selectedContact : null}
            onSubmit={handleSubmit}
            onCancel={handleCancel}
            isLoading={isLoading}
          />
        )}
      </div>

      <ConfirmDialog
        open={dialogState.isOpen}
        title={dialogState.title}
        message={dialogState.message}
        onConfirm={dialogState.onConfirm}
        onCancel={closeDialog}
        confirmLabel="Delete"
        variant="destructive"
      />
    </div>
  );
}
