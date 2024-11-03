package xpa.shadow.mylist.data.repository

import org.springframework.data.jpa.repository.JpaRepository
import org.springframework.stereotype.Repository
import xpa.shadow.mylist.data.ListItem

@Repository
interface ItemsRepository : JpaRepository<ListItem, Int>
