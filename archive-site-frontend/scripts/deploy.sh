#! /bin/sh

set -e

ng build --prod
gsutil rsync -r ./dist/archive-site-frontend gs://app-hiscribe-frontend/
gsutil setmeta  -h "Cache-control:no-store, max-age=0" gs://app-hiscribe-frontend/index.html
