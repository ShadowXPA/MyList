package xpa.shadow.mylist.data

import jakarta.persistence.*
import java.io.Serializable
import java.time.Clock
import java.time.OffsetDateTime

@Entity(name = "Lists")
class UserList : Serializable {

    @Id
    var id: Int = 0
    var name: String? = null
    var description: String? = null
    var createdAt: OffsetDateTime = OffsetDateTime.now(Clock.systemUTC())
    var updatedAt: OffsetDateTime? = null

    @OneToMany(mappedBy = "list", cascade = [CascadeType.ALL], fetch = FetchType.LAZY)
    var items: MutableList<ListItem> = mutableListOf()
}
