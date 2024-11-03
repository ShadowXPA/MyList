package xpa.shadow.mylist.data

import jakarta.persistence.*
import java.io.Serializable
import java.time.Clock
import java.time.OffsetDateTime

@Entity(name = "Items")
class ListItem : Serializable {

    @Id
    var id: Int = 0
    var name: String? = null
    var description: String? = null
    var createdAt: OffsetDateTime = OffsetDateTime.now(Clock.systemUTC())
    var updatedAt: OffsetDateTime? = null

    @ManyToOne(fetch = FetchType.LAZY)
    @JoinColumn(name = "ListId")
    var list: UserList? = null
}
