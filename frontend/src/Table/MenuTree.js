import React, { useState, useEffect, useCallback, useRef } from 'react';
import axios from 'axios';
import { useFormValidation } from '../validator/useFormValidation'; // Sửa lại đường dẫn nếu cần
import { required, maxLength } from '../validator/validators'; // Sửa lại đường dẫn nếu cần

// (Được định nghĩa bên trong file chính)
export default function MenuTree({ menus, parentId, onEdit, onDelete }) {
    // (SỬA LỖI HIỂN THỊ CON)
    const children = menus
        .filter(menu => {
            // Nếu parentId là null (đang ở gốc)
            if (parentId === null) {
                // Chấp nhận menu con có parentId là null, undefined, 0, hoặc ""
                return !menu.parentId;
            }
            // Nếu parentId là một ID (vd: 5)
            // Sử dụng so sánh lỏng (==) để khớp "5" (string) với 5 (number)
            return menu.parentId == parentId;
        })
        .sort((a, b) => a.sortOrder - b.sortOrder); // 'order' đã được map

    if (children.length === 0) {
        return null;
    }

    return (
        <ul className="menu-tree-list list-unstyled ps-4">
            {children.map(item => (
                <li key={item.id} className="menu-tree-item border-start ps-3 py-2">
                    <div className="d-flex justify-content-between align-items-center">
                        <div className="menu-item-name fw-bold">
                            {item.icon && <i className={`${item.icon} me-2`}></i>}
                            <span>{item.name}</span>
                            <span className="text-muted fs-7 ms-2">({item.url || '#'})</span>
                        </div>
                        
                        <div className="menu-item-actions">
                            <button 
                                className="btn btn-sm btn-light-primary btn-icon me-1" 
                                title="Sửa"
                                onClick={() => onEdit(item)}
                            >
                                <svg xmlns="http://www.w3.org/2000/svg" width="16" height="16" fill="currentColor" className="bi bi-pencil-fill" viewBox="0 0 16 16">
                                    <path d="M12.854.146a.5.5 0 0 0-.707 0L10.5 1.793 14.207 5.5l1.647-1.646a.5.5 0 0 0 0-.708l-3-3zm.646 6.061L9.793 2.5 3.293 9H3.5a.5.5 0 0 1 .5.5v.5h.5a.5.5 0 0 1 .5.5v.5h.5a.5.5 0 0 1 .5.5v.5h.5a.5.5 0 0 1 .5.5v.207l6.5-6.5zm-7.468 7.468A.5.5 0 0 1 6 13.5V13h-.5a.5.5 0 0 1-.5-.5V12h-.5a.5.5 0 0 1-.5-.5V11h-.5a.5.5 0 0 1-.5-.5V10h-.5a.499.499 0 0 1-.175-.032l-.179.178a.5.5 0 0 0-.11.168l-2 5a.5.5 0 0 0 .65.65l5-2a.5.5 0 0 0 .168-.11l.178-.178z"/>
                                </svg>
                            </button>
                            <button 
                                className="btn btn-sm btn-light-danger btn-icon" 
                                title="Xóa"
                                onClick={() => onDelete(item.id)}
                            >
                                <svg xmlns="http://www.w3.org/2000/svg" width="16" height="16" fill="currentColor" className="bi bi-trash-fill" viewBox="0 0 16 16">
                                    <path d="M2.5 1a1 1 0 0 0-1 1v1a1 1 0 0 0 1 1H3v9a2 2 0 0 0 2 2h6a2 2 0 0 0 2-2V4h.5a1 1 0 0 0 1-1V2a1 1 0 0 0-1-1H10a1 1 0 0 0-1-1H7a1 1 0 0 0-1 1H2.5zm3 4a.5.5 0 0 1 .5.5v7a.5.5 0 0 1-1 0v-7a.5.5 0 0 1 .5-.5zM8 5a.5.5 0 0 1 .5.5v7a.5.5 0 0 1-1 0v-7A.5.5 0 0 1 8 5zm3 .5v7a.5.5 0 0 1-1 0v-7a.5.5 0 0 1 1 0z"/>
                                </svg>
                            </button>
                        </div>
                    </div>
                    
                    <MenuTree 
                        menus={menus} 
                        parentId={item.id} 
                        onEdit={onEdit} 
                        onDelete={onDelete} 
                    />
                </li>
            ))}
        </ul>
    );
}