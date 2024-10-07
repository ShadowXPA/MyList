export interface ListItem {
  id: number,
  name: string,
  description?: string,
  createdAt: string
}

export interface UserList {
  id: number,
  name: string,
  description?: string,
  createdAt: string,
  updatedAt?: string,
  items?: ListItem[]
}
