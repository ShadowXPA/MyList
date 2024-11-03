package xpa.shadow.mylist

import org.springframework.boot.autoconfigure.SpringBootApplication
import org.springframework.boot.runApplication
import org.springframework.scheduling.annotation.EnableScheduling

@SpringBootApplication
@EnableScheduling
class MyListSearchEngineApplication

fun main(args: Array<String>) {
    runApplication<MyListSearchEngineApplication>(*args)
}
