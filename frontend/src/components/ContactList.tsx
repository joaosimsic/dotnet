import { Pencil, Trash2, Phone } from "lucide-react";
import { Button } from "./ui/button";
import { Card, CardContent } from "./ui/card";
import { ContactSkeleton } from "./ContactSkeleton";
import type { Contact } from "../types";

interface ContactListProps {
  contacts: Contact[];
  selectedId: number | null;
  onSelect: (contact: Contact) => void;
  onEdit: (contact: Contact) => void;
  onDelete: (contact: Contact) => void;
  isLoading?: boolean;
}

export function ContactList({
  contacts,
  selectedId,
  onSelect,
  onEdit,
  onDelete,
  isLoading,
}: ContactListProps) {
  if (isLoading) {
    return <ContactSkeleton count={5} />;
  }

  if (contacts.length === 0) {
    return (
      <div className="text-center py-12 text-muted-foreground">
        <p className="text-lg">No contacts found</p>
        <p className="text-sm mt-1">Add your first contact to get started</p>
      </div>
    );
  }

  return (
    <div className="space-y-2" role="list" aria-label="Contacts">
      {contacts.map((contact) => (
        <Card
          key={contact.id}
          className={`cursor-pointer transition-colors hover:bg-accent ${
            selectedId === contact.id ? "ring-2 ring-primary" : ""
          }`}
          onClick={() => onSelect(contact)}
          role="listitem"
          tabIndex={0}
          onKeyDown={(e) => {
            if (e.key === "Enter" || e.key === " ") {
              e.preventDefault();
              onSelect(contact);
            }
          }}
        >
          <CardContent className="p-4">
            <div className="flex items-center justify-between">
              <div className="flex-1 min-w-0">
                <div className="flex items-center gap-2">
                  <h3 className="font-semibold truncate">{contact.name}</h3>
                  <span className="text-sm text-muted-foreground flex-shrink-0">
                    ({contact.age} years)
                  </span>
                </div>
                <div className="flex flex-wrap gap-2 mt-1">
                  {contact.phones.map((phone) => (
                    <span
                      key={phone.id}
                      className="inline-flex items-center gap-1 text-sm text-muted-foreground"
                    >
                      <Phone className="h-3 w-3 flex-shrink-0" />
                      <span className="truncate">{phone.phoneNumber}</span>
                    </span>
                  ))}
                </div>
              </div>
              <div className="flex gap-1 flex-shrink-0 ml-2">
                <Button
                  variant="ghost"
                  size="icon"
                  onClick={(e) => {
                    e.stopPropagation();
                    onEdit(contact);
                  }}
                  aria-label={`Edit ${contact.name}`}
                >
                  <Pencil className="h-4 w-4" />
                </Button>
                <Button
                  variant="ghost"
                  size="icon"
                  onClick={(e) => {
                    e.stopPropagation();
                    onDelete(contact);
                  }}
                  aria-label={`Delete ${contact.name}`}
                >
                  <Trash2 className="h-4 w-4 text-destructive" />
                </Button>
              </div>
            </div>
          </CardContent>
        </Card>
      ))}
    </div>
  );
}
