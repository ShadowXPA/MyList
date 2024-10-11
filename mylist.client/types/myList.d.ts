export interface ListItem {
  id: number,
  name: string,
  description?: string,
  createdAt: string,
  updatedAt?: string
}

export interface UserList {
  id: number,
  name: string,
  description?: string,
  createdAt: string,
  updatedAt?: string,
  items?: ListItem[]
}
