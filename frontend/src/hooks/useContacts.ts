import { useState, useCallback } from "react";
import { api } from "@/services/api";
import type {
  Contact,
  CreateContactDto,
  UpdateContactDto,
  PagedResult,
} from "@/types";

interface UseContactsReturn {
  contacts: Contact[];
  pagination: {
    page: number;
    pageSize: number;
    totalCount: number;
    totalPages: number;
  };
  isLoading: boolean;
  error: string | null;
  loadContacts: (query?: string, page?: number) => Promise<void>;
  createContact: (data: CreateContactDto) => Promise<boolean>;
  updateContact: (id: number, data: UpdateContactDto) => Promise<boolean>;
  deleteContact: (id: number) => Promise<boolean>;
  setPage: (page: number) => void;
  clearError: () => void;
}

const DEFAULT_PAGE_SIZE = 8;

export function useContacts(): UseContactsReturn {
  const [contacts, setContacts] = useState<Contact[]>([]);
  const [pagination, setPagination] = useState({
    page: 1,
    pageSize: DEFAULT_PAGE_SIZE,
    totalCount: 0,
    totalPages: 0,
  });
  const [isLoading, setIsLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [currentQuery, setCurrentQuery] = useState("");

  const loadContacts = useCallback(async (query = "", page = 1) => {
    setIsLoading(true);
    setError(null);
    setCurrentQuery(query);

    try {
      const result: PagedResult<Contact> = query
        ? await api.searchContacts(query, page, DEFAULT_PAGE_SIZE)
        : await api.getContacts(page, DEFAULT_PAGE_SIZE);

      setContacts(result.items);
      setPagination({
        page: result.page,
        pageSize: result.pageSize,
        totalCount: result.totalCount,
        totalPages: result.totalPages,
      });
    } catch (err) {
      setError(err instanceof Error ? err.message : "Failed to load contacts");
    } finally {
      setIsLoading(false);
    }
  }, []);

  const createContact = useCallback(
    async (data: CreateContactDto): Promise<boolean> => {
      setIsLoading(true);
      setError(null);

      try {
        await api.createContact(data);
        await loadContacts(currentQuery, 1);
        return true;
      } catch (err) {
        setError(
          err instanceof Error ? err.message : "Failed to create contact",
        );
        return false;
      } finally {
        setIsLoading(false);
      }
    },
    [loadContacts, currentQuery],
  );

  const updateContact = useCallback(
    async (id: number, data: UpdateContactDto): Promise<boolean> => {
      setIsLoading(true);
      setError(null);

      try {
        await api.updateContact(id, data);
        await loadContacts(currentQuery, pagination.page);
        return true;
      } catch (err) {
        setError(
          err instanceof Error ? err.message : "Failed to update contact",
        );
        return false;
      } finally {
        setIsLoading(false);
      }
    },
    [loadContacts, currentQuery, pagination.page],
  );

  const deleteContact = useCallback(
    async (id: number): Promise<boolean> => {
      setIsLoading(true);
      setError(null);

      try {
        await api.deleteContact(id);
        await loadContacts(currentQuery, pagination.page);
        return true;
      } catch (err) {
        setError(
          err instanceof Error ? err.message : "Failed to delete contact",
        );
        return false;
      } finally {
        setIsLoading(false);
      }
    },
    [loadContacts, currentQuery, pagination.page],
  );

  const setPage = useCallback(
    (page: number) => {
      loadContacts(currentQuery, page);
    },
    [loadContacts, currentQuery],
  );

  const clearError = useCallback(() => {
    setError(null);
  }, []);

  return {
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
  };
}
