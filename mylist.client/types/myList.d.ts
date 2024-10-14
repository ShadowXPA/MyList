export interface Item {
  name: string,
  description?: string
}

export interface CurrentItem {
  id: number,
  item: Item
}

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
