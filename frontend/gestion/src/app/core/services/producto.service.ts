import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environment';
import { Producto } from '../models/producto.model';  

@Injectable({ providedIn: 'root' })
export class ProductoService {
    private readonly apiUrl = `${environment.ms_prod_url}/productos`;

    constructor(private http: HttpClient) { }

    getAll(): Observable<Producto[]> {
        return this.http.get<Producto[]>(this.apiUrl);
    }

    create(data: Producto): Observable<Producto> {
        return this.http.post<Producto>(this.apiUrl, data);
    }

    update(id: string | number, data: Producto): Observable<void> {
        const numId = +id;                       
        return this.http.put<void>(`${this.apiUrl}/${numId}`, data);
    }

    delete(id: string | number): Observable<void> {
        const numId = +id;
        return this.http.delete<void>(`${this.apiUrl}/${numId}`);
    }
}
