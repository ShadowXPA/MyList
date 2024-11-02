package xpa.shadow.mylist.config

import org.springframework.context.annotation.Bean
import org.springframework.context.annotation.Configuration
import org.springframework.jdbc.core.JdbcTemplate
import javax.sql.DataSource

@Configuration
class SqliteRepositoryConfig(private val dataSource: DataSource) {

    @Bean
    fun jdbcTemplate() = JdbcTemplate(dataSource)
}
