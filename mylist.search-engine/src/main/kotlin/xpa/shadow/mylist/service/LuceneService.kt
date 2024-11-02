package xpa.shadow.mylist.service

import org.apache.lucene.analysis.Analyzer
import org.apache.lucene.document.Document
import org.apache.lucene.document.Field
import org.apache.lucene.document.IntField
import org.apache.lucene.document.TextField
import org.apache.lucene.index.DirectoryReader
import org.apache.lucene.index.IndexWriter
import org.apache.lucene.index.IndexWriterConfig
import org.apache.lucene.index.Term
import org.apache.lucene.search.*
import org.apache.lucene.store.Directory
import org.slf4j.Logger
import org.slf4j.LoggerFactory
import org.springframework.beans.factory.annotation.Value
import org.springframework.stereotype.Service
import xpa.shadow.mylist.dto.ListItemDto
import xpa.shadow.mylist.dto.UserListDto
import java.util.regex.Pattern


@Service
class LuceneService(
    private val luceneDirectory: Directory,
    private val luceneAnalyzer: Analyzer
) {

    private val logger: Logger = LoggerFactory.getLogger(javaClass)
    private val pattern = Pattern.compile("\\s+")

    @Value("\${xpa.lucene-search.max-hits:100}")
    private val maxHits = 100

    @Value("\${xpa.lucene-search.min-score:1.0f}")
    private val minScore = 1.0f

    fun indexList(userList: UserListDto) {
        val writer = IndexWriter(luceneDirectory, IndexWriterConfig(luceneAnalyzer))
        writer.addDocument(createListDocument(userList))
        writer.close()
    }

    fun indexLists(userLists: List<UserListDto>) {
        val writer = IndexWriter(luceneDirectory, IndexWriterConfig(luceneAnalyzer))

        for (userList in userLists) {
            writer.addDocument(createListDocument(userList))
        }

        writer.close()
    }

    fun updateList(userList: UserListDto) {
        val writer = IndexWriter(luceneDirectory, IndexWriterConfig(luceneAnalyzer))
        writer.deleteDocuments(IntField.newExactQuery(UserListDto.Field.ID, userList.id))
        writer.addDocument(createListDocument(userList))
        writer.close()
    }

    fun updateLists(userLists: List<UserListDto>) {
        val writer = IndexWriter(luceneDirectory, IndexWriterConfig(luceneAnalyzer))

        for (userList in userLists) {
            writer.deleteDocuments(IntField.newExactQuery(UserListDto.Field.ID, userList.id))
            writer.addDocument(createListDocument(userList))
        }

        writer.close()
    }

    fun indexItem(listItem: ListItemDto) {
        val writer = IndexWriter(luceneDirectory, IndexWriterConfig(luceneAnalyzer))
        writer.addDocument(createItemDocument(listItem))
        writer.close()
    }

    fun indexItems(listItems: List<ListItemDto>) {
        val writer = IndexWriter(luceneDirectory, IndexWriterConfig(luceneAnalyzer))

        for (listItem in listItems) {
            writer.addDocument(createItemDocument(listItem))
        }

        writer.close()
    }

    fun updateItem(listItem: ListItemDto) {
        val writer = IndexWriter(luceneDirectory, IndexWriterConfig(luceneAnalyzer))
        writer.deleteDocuments(IntField.newExactQuery(ListItemDto.Field.ID, listItem.id))
        writer.addDocument(createItemDocument(listItem))
        writer.close()
    }

    fun updateItems(listItems: List<ListItemDto>) {
        val writer = IndexWriter(luceneDirectory, IndexWriterConfig(luceneAnalyzer))

        for (listItem in listItems) {
            writer.deleteDocuments(IntField.newExactQuery(ListItemDto.Field.ID, listItem.id))
            writer.addDocument(createItemDocument(listItem))
        }

        writer.close()
    }

    fun searchLists(query: String): List<Int> {
        if (!DirectoryReader.indexExists(luceneDirectory)) return emptyList()

        val searchQuery = getBooleanQueryBuilder(UserListDto.Field.SEARCHABLE, query).build()

        return search(searchQuery) {
            it.getField(ListItemDto.Field.LIST_ID)?.numericValue()?.toInt()
                ?: it.getField(UserListDto.Field.ID).numericValue().toInt()
        }
    }

    fun searchItems(query: String): List<Int> {
        if (!DirectoryReader.indexExists(luceneDirectory)) return emptyList()

        val searchQuery = getBooleanQueryBuilder(ListItemDto.Field.SEARCHABLE, query)
            .add(FieldExistsQuery(ListItemDto.Field.LIST_ID), BooleanClause.Occur.FILTER)
            .build()

        return search(searchQuery) { it.getField(ListItemDto.Field.ID).numericValue().toInt() }
    }

    private fun search(searchQuery: Query, getId: (doc: Document) -> Int): List<Int> {
        val idsAndScores = mutableListOf<Pair<Int, Float>>()
        val directoryReader: DirectoryReader = DirectoryReader.open(luceneDirectory)
        val indexSearcher = IndexSearcher(directoryReader)
        logger.info("Searching for documents with query: $searchQuery...")
        val topDocs = indexSearcher.search(searchQuery, maxHits)

        for (scoreDoc in topDocs.scoreDocs) {
            if (scoreDoc.score <= minScore)
                break

            val document = indexSearcher.storedFields().document(scoreDoc.doc)
            idsAndScores.add(Pair(getId(document), scoreDoc.score))
            logger.info("Found document: $document Score: ${scoreDoc.score}")
        }

        directoryReader.close()
        return createListIds(idsAndScores)
    }

    private fun createListDocument(userList: UserListDto) = Document().apply {
        add(IntField(UserListDto.Field.ID, userList.id, Field.Store.YES))
        add(TextField(UserListDto.Field.NAME, userList.name ?: "", Field.Store.NO))
        add(TextField(UserListDto.Field.DESCRIPTION, userList.description ?: "", Field.Store.NO))
    }

    private fun createItemDocument(listItem: ListItemDto) = Document().apply {
        add(IntField(ListItemDto.Field.ID, listItem.id, Field.Store.YES))
        add(TextField(ListItemDto.Field.NAME, listItem.name ?: "", Field.Store.NO))
        add(TextField(ListItemDto.Field.DESCRIPTION, listItem.description ?: "", Field.Store.NO))
        add(IntField(ListItemDto.Field.LIST_ID, listItem.listId ?: 0, Field.Store.YES))
    }

    private fun getBooleanQueryBuilder(fields: Array<String>, query: String): BooleanQuery.Builder {
        val booleanQuery = BooleanQuery.Builder()
        val terms = query.split(pattern)

        for (field in fields) {
            for (term in terms) {
                booleanQuery.add(FuzzyQuery(Term(field, term)), BooleanClause.Occur.SHOULD)
            }
        }

        return booleanQuery
    }

    /**
     * With help of mah good fwiend dormilo (A.K.A Droguilo)
     */
    private fun createListIds(array: List<Pair<Int, Float>>): List<Int> {
        val countOfEach = mutableMapOf<Int, Float>() // Number, Score
        array.forEach { countOfEach[it.first] = ((countOfEach[it.first] ?: 0F) + it.second) }
        return array.asSequence()
            .map { it.first }
            .distinct()
            .map { it to countOfEach[it] }
            .sortedByDescending { it.second }
            .map { it.first }
            .toList()
    }
}
