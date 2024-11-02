package xpa.shadow.mylist.task

import org.slf4j.Logger
import org.slf4j.LoggerFactory
import org.springframework.scheduling.annotation.Scheduled
import org.springframework.stereotype.Component
import xpa.shadow.mylist.service.ItemsService
import xpa.shadow.mylist.service.ListsService
import xpa.shadow.mylist.service.LuceneService
import java.time.Clock
import java.time.OffsetDateTime
import java.util.concurrent.TimeUnit

@Component
class LuceneIndexTask(
    private val luceneService: LuceneService,
    private val listsService: ListsService,
    private val itemsService: ItemsService
) {

    private val logger: Logger = LoggerFactory.getLogger(javaClass)
    private var lastUpdate: OffsetDateTime? = null

    @Scheduled(
        initialDelayString = "\${xpa.lucene-index.update.initial-delay:0}",
        fixedDelayString = "\${xpa.lucene-index.update.fixed-delay:15}",
        timeUnit = TimeUnit.MINUTES
    )
    fun updateIndex() {
        logger.info("Starting task to update index...")

        if (lastUpdate == null) {
            logger.info("Creating index from scratch...")

            logger.info("Indexing lists...")
            val userLists = listsService.getAllLists()
            luceneService.indexLists(userLists)
            logger.info("Lists indexed")

            logger.info("Indexing items...")
            val listItems = itemsService.getAllItems()
            luceneService.indexItems(listItems)
            logger.info("Items indexed")
        } else {
            logger.info("Updating index with items updated after the last update: $lastUpdate...")

            logger.info("Updating lists index...")
            val userLists = listsService.getListsUpdatedAfter(lastUpdate!!)
            luceneService.updateLists(userLists)
            logger.info("${userLists.size} lists updated")

            logger.info("Updating items index...")
            val listItems = itemsService.getItemsUpdatedAfter(lastUpdate!!)
            luceneService.updateItems(listItems)
            logger.info("${listItems.size} items updated")
        }

        lastUpdate = OffsetDateTime.now(Clock.systemUTC())

        logger.info("Update index task completed")
    }
}
