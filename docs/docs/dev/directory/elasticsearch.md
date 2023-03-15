# Elasticsearch

Elasticsearch is used to support the search functionality of the [Directory web app](/dev/directory).

## Prerequisites

- Elasticsearch `8.x` instance.

## Setup

### With Docker

- `docker-compose up` inside the `elastic-search/` directory will provide a suitable dev search server.
- payloads for index configuration and example queries are also in the `elastic-search/` directory.

### Without Docker

Elasticsearch can be installed locally. It depends on Java.

Kibana et al. are unnecessary for local development - Postman or similar can be used to hit the ES REST API.

## Usage

Here are some useful queries for the Elasticsearch API.

All request endpoints should be appended to the Elasticsearch server's hostname and port:

- `http://<HOST>:<PORT>/<ENDPOINT>`
  - default port is `9200`
  - for development, use `localhost`

### Warning: Content Type

All requests with bodies need the content type to be set to `application/json`.
Just one to watch out for if your tools don't default to it.

### Create the Biobanks Search Index

This needs to be done before the .NET app can interact with the index.

Request Details:

- Method: **PUT**
- Endpoint: `<SEARCH_INDEX_NAME>`

There are two separate indexes for `collections` and `capabilities`.

- Request details:
- **Capabilities**:
  - Index name: `capabilities`
  - payload: `./directory index setup/capabilities.json`
- **Collections**:
  - Index name: `collections`
  - payload: `./directory index setup/collections.json`

### Delete the Biobanks Search Index

If something's wrong with the index, or you're experimenting with different configurations, delete it and recreate it.

Request details:

- Method: **DELETE**
- Endpoint: `<SEARCH_INDEX_NAME>`

There are two separate indexes for `collections` and `capabilities`: `collections` and `capabilities` respectively.

## Health checks

### Check Cluster Health

Elasticsearch runs in a cluster of an odd number of servers. For Biobanks we have always run a cluster of one!

This request checks health of the cluster. You're mostly just looking for **GREEN**

Request details:

- Method: **GET**
- Endpoint: `_cluster/health`

### Check Index health

This will check the health of all indexes in the cluster.

- Method: **GET**
- Endpoint: `_cluster/health?level=indices&pretty=`

### Find problem shards

An ES Cluster contains several servers (or nodes), and each node can contain multiple shards. This checks shard health, so you can see if there any problematic shards.

I don't understand sharding in detail, but typically if there is a problematic shard, it can be easily resolved if you delete the index, recreate it and repopulate it.

Request details:

- Method: **GET**
- Endpoint: `_cat/shards`

### Force all indexes to use no replicas

In the past, ES has complained about only having one node in the cluster, and this has caused sharding problems, resulting in **RED** or **YELLOW** Cluster Health.

The Biobanks indexes *should* be configured to not need replica nodes (and therefore only one node is fine) when they are created.

This request will force all existing indexes not to use replica nodes, just in case.

Request details:

- Method: **PUT**
- Endpoint: `*/_settings`
  - the * is a wildcard for ALL indexes
- Payload:

  ```json
  {
    "index": {
      "number_of_replicas": 0
    }
  }
  ```

### Force future indexes to use no replicas

As above, but theoretically sets the default for new indexes.

Request details:

- Method: **PUT**
- Endpoint: `_settings`
- Payload:

  ```json
  {
    "index": {
      "number_of_replicas": 0
    }
  }
  ```

### Delete Old Marvel Indexes

This isn't a query per se, but is another use for deleting indexes.

Ok, so Marvel was a component of old Elasticsearch (2.x) used for monitoring. It was used on the old 2.x servers for Biobanks.

It logs to search indexes, one per day I think. Theoretically any Biobanks ES server actually in use should be configured to clean up these logs from both disk and the ES indexes regularly, to avoid filling up the server hard drive.

If anything goes wrong with that process, this query can remove Marvel indexes from the server (they may still need deleting from disk as well).

This is not a bad place to start if Cluster Health is **RED** or **YELLOW**.

- So, you can use the **DELETE Index** query as above, instead of using the Biobanks index name(s), you would use the name of Marvel indexes.
- Use **Get Indexes Health** query to find the names of non Biobanks indexes you might be interested in deleting
- Refer to ES docs to see how you can put wildcards in index names to delete multiple matching indexes at once.
  - As I said, I think they're daily, and their names contain a datestamp. So you could clear out a whole month or a whole year by wildcarding something like `2020-06-*` etc.
