import { HttpOptions, ODataEntityResource, ODataEntitySetResource } from 'angular-odata/lib/resources';
import { EntityKey } from 'angular-odata/lib/types';
import { ODataEntityConfig } from 'angular-odata/lib/configs/entity';
import { Observable } from 'rxjs';
import { ODataApiConfig, ODataServiceConfig } from 'angular-odata';

export interface IDataEntityService<T> {
  apiConfig: ODataApiConfig;
  serviceConfig: ODataServiceConfig;
  entityConfig: ODataEntityConfig<T>;

  entities(): ODataEntitySetResource<T>;

  entity(key?: EntityKey<T>): ODataEntityResource<T>;

  create(entity: Partial<T>, options?: HttpOptions): Observable<T>;

  update(entity: Partial<T>, options?: HttpOptions): Observable<T>;

  assign(entity: Partial<T>, attrs: Partial<T>, options?: HttpOptions): Observable<T>;

  destroy(entity: Partial<T>, options?: HttpOptions): Observable<any>;
}
