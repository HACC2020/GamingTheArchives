import { IDataEntityService } from 'src/app/services/data-entity-service';
import {
  EntityKey,
  HttpOptions,
  ODataApiConfig,
  ODataEntityConfig, ODataEntityResource, ODataEntityService,
  ODataEntitySetResource,
  ODataServiceConfig
} from 'angular-odata';
import { Observable } from 'rxjs';

export class DataEntityServiceWrapper<T> implements IDataEntityService<T> {
  get apiConfig(): ODataApiConfig {
    return this.service.apiConfig;
  }

  get entityConfig(): ODataEntityConfig<T> {
    return this.service.entityConfig;
  }

  get serviceConfig(): ODataServiceConfig {
    return this.service.serviceConfig;
  }

  constructor(protected service: ODataEntityService<T>) {
  }

  entities(): ODataEntitySetResource<T> {
    return this.service.entities();
  }

  entity(key?: EntityKey<T>): ODataEntityResource<T> {
    return this.service.entity(key);
  }

  create(entity: Partial<T>, options?: HttpOptions): Observable<T> {
    return this.service.create(entity, options);
  }

  update(entity: Partial<T>, options?: HttpOptions): Observable<T> {
    return this.service.update(entity, options);
  }

  assign(entity: Partial<T>, attrs: Partial<T>, options?: HttpOptions): Observable<T> {
    return this.service.assign(entity, attrs, options);
  }

  destroy(entity: Partial<T>, options?: HttpOptions): Observable<any> {
    return this.service.destroy(entity, options);
  }

}
