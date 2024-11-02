package xpa.shadow.mylist.dto

import java.time.OffsetDateTime

class UserListDto(
    val id: Int,
    val name: String?,
    val description: String?,
    var createdAt: OffsetDateTime,
    var updatedAt: OffsetDateTime?,
    var items: List<ListItemDto>? = null
) {

    class Field {
        companion object {
            val ID = "id"
            val NAME = "name"
            val DESCRIPTION = "description"
            val SEARCHABLE = arrayOf(NAME, DESCRIPTION)
        }
    }
}
