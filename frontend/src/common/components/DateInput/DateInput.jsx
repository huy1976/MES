import React, { useState } from 'react';
import { FaCalendarAlt } from 'react-icons/fa';
import clsx from 'clsx';
import styles from './DateInput.module.scss'

const DateInput = ({ 
  selectedDate, 
  onChange = () => {}, 
  placeholder = "Chọn ngày", 
  className = "", 
  style
}) => {
  const [isCalendarOpen, setIsCalendarOpen] = useState(false);
  
  const toggleCalendar = () => {
    setIsCalendarOpen(!isCalendarOpen);
  };
  
  const handleDateChange = (date) => {
    onChange(date);
    setIsCalendarOpen(false);
  };

  // 1. Định dạng để hiển thị ra màn hình (DD/MM/YYYY HH:mm)
  const formatDateForDisplay = (date) => {
    if (!date) return '';
    const day = String(date.getDate()).padStart(2, '0');
    const month = String(date.getMonth() + 1).padStart(2, '0');
    const year = date.getFullYear();
    const hours = String(date.getHours()).padStart(2, '0');
    const minutes = String(date.getMinutes()).padStart(2, '0');
    return `${day}/${month}/${year} ${hours}:${minutes}`;
  };

  // 2. Định dạng chuẩn ISO để input datetime-local có thể đọc được
  const formatDateForInput = (date) => {
    if (!date) return '';
    const year = date.getFullYear();
    const month = String(date.getMonth() + 1).padStart(2, '0');
    const day = String(date.getDate()).padStart(2, '0');
    const hours = String(date.getHours()).padStart(2, '0');
    const minutes = String(date.getMinutes()).padStart(2, '0');
    return `${year}-${month}-${day}T${hours}:${minutes}`;
  };

  return (
    <div className={clsx(styles.dateInputContainer, className)} style={style}>
      {/* Lớp hiển thị: Sẽ đè lên input thật để hiện định dạng DD/MM/YYYY */}
      <div className={styles.displayOverlay}>
        <span className={styles.text}>
          {selectedDate ? formatDateForDisplay(selectedDate) : placeholder}
        </span>
        <FaCalendarAlt className={styles.icon} />
      </div>

      {/* Input thật: Được làm trong suốt nhưng vẫn nằm trên cùng để nhận click */}
      <input
        type="datetime-local"
        value={formatDateForInput(selectedDate)}
        onChange={(e) => handleDateChange(new Date(e.target.value))}
        className={styles.hiddenInput}
        onClick={toggleCalendar}
      />
    </div>
  );
};

export default DateInput;