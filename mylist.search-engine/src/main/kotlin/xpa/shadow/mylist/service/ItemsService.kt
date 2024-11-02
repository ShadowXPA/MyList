package xpa.shadow.mylist.service

import org.springframework.stereotype.Service
import org.springframework.transaction.annotation.Transactional
import xpa.shadow.mylist.data.repository.ItemsRepository
import xpa.shadow.mylist.data.repository.RepositoryRaw
import xpa.shadow.mylist.dto.ListItemDto
import xpa.shadow.mylist.dto.UserListDto
import java.time.OffsetDateTime

@Service
class ItemsService(
    private val itemsRepository: ItemsRepository,
    private val repositoryRaw: RepositoryRaw,
    private val luceneService: LuceneService
) {

    @Transactional(readOnly = true)
    fun getAllItems(): List<ListItemDto> {
        return itemsRepository.findAll().map { item ->
            ListItemDto(item.id, item.name, item.description, item.createdAt, item.updatedAt, item.list?.id)
        }
    }

    @Transactional(readOnly = true)
    fun getItemsUpdatedAfter(updatedAt: OffsetDateTime): List<ListItemDto> {
        return repositoryRaw.findAllItemsByUpdatedAtAfter(updatedAt).map { item ->
            ListItemDto(item.id, item.name, item.description, item.createdAt, item.updatedAt, item.list?.id)
        }
    }

    fun searchItems(query: String?): List<Int> {
        return if (query.isNullOrBlank()) {
            getAllItems().map { it.id }
        } else {
            luceneService.searchItems(query)
        }
    }
}
