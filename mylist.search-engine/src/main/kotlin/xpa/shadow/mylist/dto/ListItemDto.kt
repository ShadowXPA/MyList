package xpa.shadow.mylist.dto

import java.time.OffsetDateTime

class ListItemDto(
    var id: Int,
    var name: String?,
    var description: String?,
    var createdAt: OffsetDateTime,
    var updatedAt: OffsetDateTime?,
    var listId: Int?
) {

    class Field {
        companion object {
            val ID = "id"
            val NAME = "name"
            val DESCRIPTION = "description"
            val LIST_ID = "listId"
            val SEARCHABLE = arrayOf(NAME, DESCRIPTION)
        }
    }
}
