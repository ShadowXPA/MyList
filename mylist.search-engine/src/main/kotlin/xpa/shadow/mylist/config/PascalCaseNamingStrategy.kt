package xpa.shadow.mylist.config

import org.hibernate.boot.model.naming.Identifier
import org.hibernate.boot.model.naming.PhysicalNamingStrategy
import org.hibernate.engine.jdbc.env.spi.JdbcEnvironment

class PascalCaseNamingStrategy : PhysicalNamingStrategy {

    override fun toPhysicalColumnName(name: Identifier?, context: JdbcEnvironment?): Identifier {
        val pascalCase = name?.text?.split('_')
            ?.joinToString("") { it.replaceFirstChar { char -> char.uppercase() } } ?: ""
        return Identifier.toIdentifier(pascalCase)
    }

    override fun toPhysicalTableName(name: Identifier?, context: JdbcEnvironment?): Identifier {
        val pascalCase = name?.text?.split('_')
            ?.joinToString("") { it.replaceFirstChar { char -> char.uppercase() } } ?: ""
        return Identifier.toIdentifier(pascalCase)
    }

    override fun toPhysicalSchemaName(name: Identifier?, context: JdbcEnvironment?) = name
    override fun toPhysicalCatalogName(name: Identifier?, context: JdbcEnvironment?) = name
    override fun toPhysicalSequenceName(name: Identifier?, context: JdbcEnvironment?) = name
}