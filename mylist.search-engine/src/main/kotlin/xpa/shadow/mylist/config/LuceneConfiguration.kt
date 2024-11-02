package xpa.shadow.mylist.config

import org.apache.lucene.analysis.Analyzer
import org.apache.lucene.analysis.CharArraySet
import org.apache.lucene.analysis.core.WhitespaceAnalyzer
import org.apache.lucene.analysis.standard.StandardAnalyzer
import org.apache.lucene.store.ByteBuffersDirectory
import org.apache.lucene.store.Directory
import org.springframework.context.annotation.Bean
import org.springframework.context.annotation.Configuration

@Configuration
class LuceneConfiguration {

    @Bean
    fun luceneDirectory(): Directory {
        return ByteBuffersDirectory()
    }

    @Bean
    fun luceneAnalyzer(): Analyzer {
        val charArraySet = CharArraySet(16, true)
//        charArraySet.addAll(listOf("to"))
        return StandardAnalyzer(charArraySet)
    }
}
