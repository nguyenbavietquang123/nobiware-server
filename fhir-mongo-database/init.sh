#!/bin/bash
echo "⏳ Waiting for MongoDB to start..."
sleep 5

echo "⚙️ Restoring dump into database 'spark'..."
mongorestore --db spark /backup/spark || echo "❗ mongorestore failed or data already exists"

echo "✅ Restore done."
