package xpa.shadow.mylist.controller

import org.springframework.web.bind.annotation.GetMapping
import org.springframework.web.bind.annotation.PathVariable
import org.springframework.web.bind.annotation.RequestMapping
import org.springframework.web.bind.annotation.RestController
import xpa.shadow.mylist.dto.ListItemDto
import xpa.shadow.mylist.dto.UserListDto
import xpa.shadow.mylist.service.ItemsService
import xpa.shadow.mylist.service.ListsService

@RestController
@RequestMapping("/api")
class SearchController(private val listsService: ListsService, private val itemsService: ItemsService) {

    @GetMapping("/lists")
    fun getAllLists(): List<UserListDto> = listsService.getAllLists()

    @GetMapping("/lists/{id}/items")
    fun getAllListItems(@PathVariable("id") id: Int): List<ListItemDto> = listsService.getAllItems(id)

    @GetMapping("/items")
    fun getAllItems(): List<ListItemDto> = itemsService.getAllItems()

    @GetMapping("/search/lists")
    fun searchLists(q: String?): List<Int> = listsService.searchLists(q)

    @GetMapping("/search/items")
    fun searchItems(q: String?): List<Int> = itemsService.searchItems(q)
}
