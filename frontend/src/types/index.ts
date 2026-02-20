export interface Phone {
  id: number;
  phoneNumber: string;
}

export interface Contact {
  id: number;
  name: string;
  age: number;
  createdAt: string;
  updatedAt: string;
  phones: Phone[];
}

export interface PagedResult<T> {
  items: T[];
  totalCount: number;
  page: number;
  pageSize: number;
  totalPages: number;
}

export interface CreateContactDto {
  name: string;
  age: number;
  phoneNumbers: string[];
}

export interface UpdateContactDto {
  name: string;
  age: number;
  phoneNumbers: string[];
}
