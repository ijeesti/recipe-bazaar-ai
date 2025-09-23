

# Recipe Bazaar AI: A Modern Approach to Search

This repository is a practical, fully-containerized demonstration of how to build a powerful, AI-powered search solution that effortlessly scales and integrates with your existing relational databases. It's a living example of how to move beyond manual, complex SQL queries.

🚀 Key Features \& Highlights

Automated Setup: The application automatically creates and configures the search index and indexer from your SQL database at runtime. No manual setup is required!
Intelligent Search APIs: Explore a suite of modern API endpoints built to handle complex search scenarios with minimal code.
Seamless Integration: This project demonstrates a clean architecture that separates your transactional database from your search layer for improved performance and scalability.

🔍 API Endpoints in Action

This project includes a comprehensive API to showcase a superior search experience.



| Feature           | Description                                                                 |

|-------------------|-----------------------------------------------------------------------------|

| Full-Text Search  | Search across all fields and get relevance-ranked results.                  |

| Autocomplete      | Get instant, type-ahead suggestions with typo tolerance.                    |

| Weighted Search   | Prioritize results by field importance (e.g., Title over Description).       |

| Real-Time Updates | Add new comments that are instantly searchable via real-time indexing.       |


⚡ Getting Started with Docker

The entire project is containerized for a friction-free setup. Just follow these two steps to get it running locally.
Clone the Repository:

```bash
git clone https://github.com/ijeesti/recipe-bazaar-ai.git
cd recipe-bazaar-ai
docker-compose up --build

```
👉 Once the containers are up, open your browser and navigate to Swagger UI for an interactive API documentation and testing experience.
```

### Contributing

Pull requests are welcome. For major changes, please open an issue first

to discuss what you would like to change.





