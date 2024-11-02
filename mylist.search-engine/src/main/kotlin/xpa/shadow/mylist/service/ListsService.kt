package xpa.shadow.mylist.service

import org.springframework.stereotype.Service
import org.springframework.transaction.annotation.Transactional
import xpa.shadow.mylist.data.repository.ListsRepository
import xpa.shadow.mylist.data.repository.RepositoryRaw
import xpa.shadow.mylist.dto.ListItemDto
import xpa.shadow.mylist.dto.UserListDto
import java.time.OffsetDateTime

@Service
class ListsService(
    private val listsRepository: ListsRepository,
    private val repositoryRaw: RepositoryRaw,
    private val luceneService: LuceneService
) {

    @Transactional(readOnly = true)
    fun getAllLists(): List<UserListDto> {
        return listsRepository.findAll().map { list ->
            UserListDto(list.id, list.name, list.description, list.createdAt, list.updatedAt, list.items.map { item ->
                ListItemDto(item.id, item.name, item.description, item.createdAt, item.updatedAt, list.id)
            })
        }
    }

    @Transactional(readOnly = true)
    fun getAllItems(id: Int): List<ListItemDto> {
        return listsRepository.findById(id).orElseThrow().items.map { item ->
            ListItemDto(item.id, item.name, item.description, item.createdAt, item.updatedAt, item.list?.id)
        }
    }

    @Transactional(readOnly = true)
    fun getListsUpdatedAfter(updatedAt: OffsetDateTime): List<UserListDto> {
        return repositoryRaw.findAllListsByUpdatedAtAfter(updatedAt).map { list ->
            UserListDto(list.id, list.name, list.description, list.createdAt, list.updatedAt)
        }
    }

    fun searchLists(query: String?): List<Int> {
        return if (query.isNullOrBlank()) {
            getAllLists().map { it.id }
        } else {
            luceneService.searchLists(query)
        }
    }
}
