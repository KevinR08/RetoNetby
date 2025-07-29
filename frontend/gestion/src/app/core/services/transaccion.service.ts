// src/app/core/services/transaccion.service.ts
import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Transaccion } from '../models/transaccion.model';
import { environment } from '../../environment';

@Injectable({ providedIn: 'root' })
export class TransaccionService {
    private readonly api = `${environment.ms_transac_url}/Transacciones`;

    constructor(private http: HttpClient) { }

    getAll(params?: {
    tipo?: string;
    inicio?: string;
    fin?: string;
    productoId?: number;
}): Observable<Transaccion[]> {
    let query = new HttpParams();
    if (params?.tipo) query = query.set('tipo', params.tipo);
    if (params?.inicio) query = query.set('inicio', params.inicio);
    if (params?.fin) query = query.set('fin', params.fin);
    if (params?.productoId !== undefined) query = query.set('productoId', params.productoId.toString());

    return this.http.get<Transaccion[]>(this.api, { params: query });
}


    update(dto: Transaccion): Observable<Transaccion> {
        return this.http.put<Transaccion>(`${this.api}/${dto.id}`, dto);
    }

    create(dto: Transaccion): Observable<Transaccion> {
        return this.http.post<Transaccion>(this.api, dto);
    }

    delete(id: number): Observable<void> {
        return this.http.delete<void>(`${this.api}/${id}`);
    }
}
