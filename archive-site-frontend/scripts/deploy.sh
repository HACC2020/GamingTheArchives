#! /bin/sh

set -e

ng build --prod
gsutil rsync -r ./dist/archive-site-frontend gs://app-hiscribe-frontend/
