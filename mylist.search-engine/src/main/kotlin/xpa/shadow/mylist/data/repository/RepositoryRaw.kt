package xpa.shadow.mylist.data.repository

import org.springframework.jdbc.core.JdbcTemplate
import org.springframework.jdbc.core.RowMapper
import org.springframework.stereotype.Repository
import xpa.shadow.mylist.data.ListItem
import xpa.shadow.mylist.data.UserList
import java.time.OffsetDateTime
import java.time.ZoneOffset

@Repository
class RepositoryRaw(private val jdbcTemplate: JdbcTemplate) {

    private val listRowMapper = RowMapper<UserList> { rs, _ ->
        UserList().apply {
            id = rs.getInt("Id")
            name = rs.getString("Name")
            description = rs.getString("Description")
            createdAt = rs.getTimestamp("CreatedAt")!!.toLocalDateTime().atOffset(ZoneOffset.UTC)
            updatedAt = rs.getTimestamp("UpdatedAt")?.toLocalDateTime()?.atOffset(ZoneOffset.UTC)
        }
    }

    private val itemRowMapper = RowMapper<ListItem> { rs, _ ->
        ListItem().apply {
            id = rs.getInt("Id")
            name = rs.getString("Name")
            description = rs.getString("Description")
            createdAt = rs.getTimestamp("CreatedAt")!!.toLocalDateTime().atOffset(ZoneOffset.UTC)
            updatedAt = rs.getTimestamp("UpdatedAt")?.toLocalDateTime()?.atOffset(ZoneOffset.UTC)
            list = UserList().apply {
                id = rs.getInt("ListId")
            }
        }
    }

    fun findAllListsByUpdatedAtAfter(updatedAt: OffsetDateTime): List<UserList> {
        val sql = "SELECT * FROM Lists WHERE UpdatedAt > DATETIME(?)"
        return jdbcTemplate.query(sql, listRowMapper, updatedAt)
    }

    fun findAllItemsByUpdatedAtAfter(updatedAt: OffsetDateTime): List<ListItem> {
        val sql = "SELECT * FROM Items WHERE UpdatedAt > DATETIME(?)"
        return jdbcTemplate.query(sql, itemRowMapper, updatedAt)
    }
}
