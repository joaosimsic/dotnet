import type {
  Contact,
  CreateContactDto,
  UpdateContactDto,
  PagedResult,
} from "../types";

const API_URL = import.meta.env.VITE_API_URL || "http://localhost:5000";
const API_VERSION = "v1";

async function handleResponse<T>(response: Response): Promise<T> {
  if (!response.ok) {
    const error = await response.text();
    throw new Error(error || `HTTP error! status: ${response.status}`);
  }
  return response.json();
}

export const api = {
  async getContacts(page = 1, pageSize = 10): Promise<PagedResult<Contact>> {
    const response = await fetch(
      `${API_URL}/api/${API_VERSION}/contacts?page=${page}&pageSize=${pageSize}`,
    );
    return handleResponse<PagedResult<Contact>>(response);
  },

  async getContact(id: number): Promise<Contact> {
    const response = await fetch(
      `${API_URL}/api/${API_VERSION}/contacts/${id}`,
    );
    return handleResponse<Contact>(response);
  },

  async searchContacts(
    query: string,
    page = 1,
    pageSize = 10,
  ): Promise<PagedResult<Contact>> {
    const response = await fetch(
      `${API_URL}/api/${API_VERSION}/contacts/search?q=${encodeURIComponent(query)}&page=${page}&pageSize=${pageSize}`,
    );
    return handleResponse<PagedResult<Contact>>(response);
  },

  async createContact(data: CreateContactDto): Promise<Contact> {
    const response = await fetch(`${API_URL}/api/${API_VERSION}/contacts`, {
      method: "POST",
      headers: { "Content-Type": "application/json" },
      body: JSON.stringify(data),
    });
    return handleResponse<Contact>(response);
  },

  async updateContact(id: number, data: UpdateContactDto): Promise<Contact> {
    const response = await fetch(
      `${API_URL}/api/${API_VERSION}/contacts/${id}`,
      {
        method: "PUT",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify(data),
      },
    );
    return handleResponse<Contact>(response);
  },

  async deleteContact(id: number): Promise<Contact> {
    const response = await fetch(
      `${API_URL}/api/${API_VERSION}/contacts/${id}`,
      {
        method: "DELETE",
      },
    );
    return handleResponse<Contact>(response);
  },

  async checkHealth(): Promise<boolean> {
    try {
      const response = await fetch(`${API_URL}/health`);
      return response.ok;
    } catch {
      return false;
    }
  },
};
