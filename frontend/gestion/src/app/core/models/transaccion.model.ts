// src/app/core/models/transaccion.model.ts
export interface Transaccion {
  id?: number;
  fecha: string; // formato ISO
  tipo: 'Compra' | 'Venta';
  productoId: number;
  cantidad: number;
  precioUnitario: number;
  detalle?: string;
}
