package xpa.shadow.mylist.data.repository

import org.springframework.data.jpa.repository.JpaRepository
import org.springframework.stereotype.Repository
import xpa.shadow.mylist.data.UserList
import java.time.OffsetDateTime

@Repository
interface ListsRepository : JpaRepository<UserList, Int> {

    // This doesn't work for whatever reason...
    fun findAllByUpdatedAtAfter(updatedAt: OffsetDateTime): List<UserList>
}
